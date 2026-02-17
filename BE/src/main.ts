import express = require("express");


const app = express();

app.get("/health", (_req, res) => {
  res.json({ ok: true });
});

app.listen(4000, () => {
  console.log("Backend running on http://localhost:4000");
});