<template>
  <component 
    :is="inline ? 'span' : 'div'" 
    :class="['formula-renderer', className]" 
    v-html="renderedHTML"
  />
</template>

<script setup lang="ts">
import { computed } from 'vue'
import MarkdownIt from 'markdown-it'
import katex from 'katex'
import 'katex/dist/katex.min.css'

interface Props {
  content: string
  inline?: boolean
  className?: string
}

const props = withDefaults(defineProps<Props>(), {
  inline: false,
  className: ''
})

const md = new MarkdownIt({
  html: true,
  linkify: true,
  typographer: true
})

const renderedHTML = computed(() => {
  if (!props.content) return ''

  let text = props.content
  const mathTokens: string[] = []

  // 1. Trích xuất công thức khối $$...$$ và thay thế bằng token tạm thời
  text = text.replace(/\$\$([\s\S]+?)\$\$/g, (_, formula) => {
    try {
      const rendered = katex.renderToString(formula.trim(), {
        displayMode: true,
        throwOnError: false
      })
      const token = `%%MATH_TOKEN_${mathTokens.length}%%`
      mathTokens.push(rendered)
      return token
    } catch (err) {
      return `$$${formula}$$`
    }
  })

  // 2. Trích xuất công thức dòng $...$ và thay thế bằng token tạm thời
  text = text.replace(/\$([^\$\n]+?)\$/g, (_, formula) => {
    try {
      const rendered = katex.renderToString(formula.trim(), {
        displayMode: false,
        throwOnError: false
      })
      const token = `%%MATH_TOKEN_${mathTokens.length}%%`
      mathTokens.push(rendered)
      return token
    } catch (err) {
      return `$${formula}$`
    }
  })

  // 3. Render Markdown từ văn bản đã tách công thức
  let htmlResult = md.render(text)

  mathTokens.forEach((mathHtml, index) => {
    const token = `%%MATH_TOKEN_${index}%%`
    htmlResult = htmlResult.replace(token, mathHtml)
  })

  return htmlResult
})
</script>

<style scoped>
.formula-renderer :deep(.katex-display) {
  margin: 0.75rem 0;
  overflow-x: auto;
  overflow-y: hidden;
  padding-bottom: 0.25rem;
}
.formula-renderer :deep(.katex) {
  font-size: 1.05em;
  line-height: 1.15;
}
</style>
