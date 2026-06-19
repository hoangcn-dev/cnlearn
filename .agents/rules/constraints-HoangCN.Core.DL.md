---
trigger: always_on
---

# Constraints of Project HoangCN.Core.DL

## General Project Constraints
- Only allowed to reference `HoangCN.Core.Common`.
- Business logic or calling back to the BL/API layers is strictly prohibited.

## Detailed Constraints by Folder

### 2.1 Interfaces
- Only contains asynchronous method declarations (async methods).

### 2.2 Implementation
- Must ensure database connections are released after use and manage transactions strictly.

### 2.3 DynamicDbContext
- Must inherit from EF Core's `DbContext` and set up secure connection configurations.
