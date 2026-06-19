---
trigger: always_on
---

# Constraints of Project HoangCN.Core.Common

## General Project Constraints
- Referencing any other projects in the Solution is strictly prohibited.
- Absolute independence must be maintained so that all other projects can reference it.

## Detailed Constraints by Folder

### 1.1 Attributes
- Only contains Attribute declarations; complex processing logic is prohibited.

### 1.2 Base
- Every Entity in the system must inherit from `BaseEntity` to ensure database synchronization and audit logging.

### 1.3 Enums
- Only contains Enum declarations.

### 1.4 Exceptions
- Must inherit from the system's base Exception class; returning raw error codes or null to clients on errors is prohibited.

### 1.5 Model
- Only contains properties for data storage; business or processing logic is strictly prohibited.

### 1.6 Utils
- Must be `static` functions, independent of DB state or Context changes.
