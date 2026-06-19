---
trigger: always_on
---

# Constraints of Project HoangCN.Core.BL

## General Project Constraints
- Must reference `HoangCN.Core.Common` and `HoangCN.Core.DL`.

## Detailed Constraints by Folder

### 3.1 Base
- Must be a generic class inheriting from `IBaseBL<TEntity>`. Use `_baseReadDL` for reading data and `_baseWriteDL` for writing data.

### 3.2 Metadata
- Only serves the dynamic SQL generation process; must not contain business logic.

### 3.3 Utils
- Must generate safe SQL statements (using Dapper parameterization) to prevent SQL Injection.
