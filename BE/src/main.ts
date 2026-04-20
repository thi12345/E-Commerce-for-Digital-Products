import "dotenv/config"; 
import express from "express";
import productRoutes from "./presentation/http/routes/product.routes"
import { AppDataSource } from "./infrastructure/db/postgres";

const app = express();
app.use(express.json());

// 👇 your API prefix
app.use("/api", productRoutes);

const PORT = 3000;

AppDataSource.initialize()
  .then(() => {
    console.log("DB connected");

    app.listen(PORT, () => {
      console.log(`Server running on http://localhost:${PORT}`);
    });
  })
  .catch((err) => console.error("DB error:", err));