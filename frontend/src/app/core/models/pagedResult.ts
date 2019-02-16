export class PagedResult<T> {
    public constructor(
        public pageIndex: number,
        public pageCount: number,
        public pageSize: number,
        public totalDataRowsCount: number,
        public dataRows: T[]
    ){}
}