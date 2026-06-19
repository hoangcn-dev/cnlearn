export enum QuestionLevel {
    Easy = 0,
    Medium = 1,
    Hard = 2
}

export const QuestionLevelLabel: Record<QuestionLevel, string> = {
    [QuestionLevel.Easy]: 'Dễ',
    [QuestionLevel.Medium]: 'Trung bình',
    [QuestionLevel.Hard]: 'Khó',
};

export enum QuestionType {
    SingleChoice = 0,
    MultipleChoice = 1
}

export const QuestionTypeLabel: Record<QuestionType, string> = {
    [QuestionType.SingleChoice]: 'Chọn 1 đáp án',
    [QuestionType.MultipleChoice]: 'Chọn nhiều đáp án',
};

export enum QuestionAccessType {
    Public = 0,
    Private = 1
}

export const QuestionAccessTypeLabel: Record<QuestionAccessType, string> = {
    [QuestionAccessType.Public]: 'Công khai',
    [QuestionAccessType.Private]: 'Cá nhân',
};

export interface BankQuestionDto {
    questionId: string;
    questionSlug?: string;
    stringContent?: string;
    explaination?: string;
    level: QuestionLevel;
    type: QuestionType;
    accessType: QuestionAccessType;
    questionCategoryName: string;
    questionCategoryId: string;
    attemptCount: number;
    learnMsUserId: string;
    fullName: string;
    isInBank: boolean;
    modifiedDate: Date;
}

export interface BankAnswerDto {
    questionAnswerId: string;
    stringContent?: string;
    isCorrectAnswer: boolean;
}

export interface BankQuestionWithAnswersDto extends BankQuestionDto {
    answers: BankAnswerDto[];
}
