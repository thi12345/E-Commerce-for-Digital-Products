import { Router } from "express";
import { ProductController } from "../controllers/ProductController";

const router = Router();
const controller = new ProductController();
//Get Product
router.get("/products", controller.getProducts.bind(controller));

//  CREATE Product
router.post("/products", controller.createProduct.bind(controller));
export default router;
