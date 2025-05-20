export interface Pagination<T> {
    data: T[];
    currentPage: number;
    size: number;
    total: number;
}