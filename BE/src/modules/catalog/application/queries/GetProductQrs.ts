export class GetProductQrs{
    constructor(
        public readonly minPrice?: number,
        public readonly keyword?: string
    ) {}
}