import { Request, Response } from "express";
import { GetProductQrsHandler } from "../../../modules/catalog/application/handlers/GetProductQrsHandler";
import { GetProductQrs } from "../../../modules/catalog/application/queries/GetProductQrs";
import { AppDataSource } from "../../../infrastructure/db/postgres";
import { CreateProductCmd } from "../../../modules/catalog/application/commands/CreateProductCmd";
import { CreateProductCmdHandler } from "../../../modules/catalog/application/handlers/CreateProductCmdHandler";

export class ProductController {
  private handler: GetProductQrsHandler;
    private createHandler: CreateProductCmdHandler;
  constructor() {
    this.handler = new GetProductQrsHandler(AppDataSource);
    this.createHandler = new CreateProductCmdHandler(AppDataSource)
  }
  //getProduct
  async getProducts(req: Request, res: Response): Promise<Response> {
    try {
      const query = new GetProductQrs();
      const products = await this.handler.handle(query);

      return res.status(200).json({
        success: true,
        data: products,
      });
    } catch (error) {
      console.error("Error fetching products:", error);

      return res.status(500).json({
        success: false,
        message: "Internal server error",
      });
    }
  }
    async createProduct(req: Request, res: Response): Promise<Response> {
    try {
      const body: CreateProductCmd= req.body;

      //  basic validation
      if (!body.price || !body.description) {
        return res.status(400).json({
          success: false,
          message: "price and description are required",
        });
      }

      //  request → command
      const command = new CreateProductCmd({
        price: body.price,
        description: body.description,
      });

      const product = await this.createHandler.handle(command);

      return res.status(201).json({
        success: true,
        data: product,
      });
    } catch (error: any) {
      console.error(error);

      return res.status(500).json({
        success: false,
        message: error.message || "Internal server error",
      });
    }
  }
}