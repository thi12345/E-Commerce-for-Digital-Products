import { DataSource } from "typeorm";
import { Product } from "../../domain/entities/Product";
import { CreateProductCmd } from "../commands/CreateProductCmd";

export class CreateProductCmdHandler {
  constructor(private readonly dataSource: DataSource) {}

  async handle(command: CreateProductCmd): Promise<Product> {
    const repo = this.dataSource.getRepository(Product);

    const product = repo.create({
      price: command.price,
      description: command.description,
    });

    return await repo.save(product);
  }
}