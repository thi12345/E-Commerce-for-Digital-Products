export class CreateProductCmd {
  public readonly price: number;
  public readonly description: string;

  constructor(props: { price: number; description: string }) {
      if (props.price <= 0) {
      throw new Error("Price must be greater than 0");
    }

    if (!props.description || props.description.length < 3) {
      throw new Error("Description too short");
    }

    this.price = props.price;
    this.description = props.description;
  }

}