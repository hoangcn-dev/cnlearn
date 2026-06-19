export interface GetRequest {
    page?: number;
    size?: number;
    sortBy?: string;
    isAsc?: boolean;
    isPaging: boolean;
    key?: string;
    ids?: string[];
    filters: Filter[];
    filterGroupType: FilterGroupType;
}

export enum FilterGroupType {
    And = 0,
    Or = 1
}

export const FilterGroupTypeLabel: Record<FilterGroupType, string> = {
    [FilterGroupType.And]: 'And',
    [FilterGroupType.Or]: 'Or',
};

export interface Filter {
    property: string;
    operator: FilterOperator;
    value?: any;
    type: FilterType;
    columnToCompare?: string;
}

export enum FilterOperator {
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    Contain,
    NotContain,
    StartWith,
    EndWith,
    In,
    NotIn
}

export const FilterOperatorLabel: Record<FilterOperator, string> = {
    [FilterOperator.Equal]: 'Bằng',
    [FilterOperator.NotEqual]: 'Không bằng',
    [FilterOperator.LessThan]: 'Nhỏ hơn',
    [FilterOperator.LessThanOrEqual]: 'Nhỏ hơn hoặc bằng',
    [FilterOperator.GreaterThan]: 'Lớn hơn',
    [FilterOperator.GreaterThanOrEqual]: 'Lớn hơn hoặc bằng',
    [FilterOperator.Contain]: 'Chứa',
    [FilterOperator.NotContain]: 'Không chứa',
    [FilterOperator.StartWith]: 'Bắt đầu với',
    [FilterOperator.EndWith]: 'Kết thúc với',
    [FilterOperator.In]: 'Thuộc',
    [FilterOperator.NotIn]: 'Không thuộc',
};

export enum FilterType {
    Number,
    String,
    Bool,
    Date
}

export const FilterTypeLabel: Record<FilterType, string> = {
    [FilterType.Number]: 'Số',
    [FilterType.String]: 'Chuỗi',
    [FilterType.Bool]: 'Boolean',
    [FilterType.Date]: 'Ngày',
};
