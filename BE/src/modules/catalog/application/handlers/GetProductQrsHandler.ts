import { DataSource } from "typeorm";
import { Product } from "../../domain/entities/Product";
import { GetProductQrs } from "../queries/GetProductQrs";


export class GetProductQrsHandler{
    constructor(
        private readonly dataSource: DataSource
    ) {}

    async handle(query: GetProductQrs): Promise<Product[]> {
        const productRepository = this.dataSource.getRepository(Product);
        return productRepository.find();
    }

}