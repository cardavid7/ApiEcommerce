export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  imgUrl: string | null;
  imgUrlLocal: string | null;
  sku: string;
  stock: number;
  creationDate: string;
  updateDate: string | null;
  categoryId: number;
  categoryName: string;
}

// Create/Update se mandan como multipart/form-data (el backend acepta imagen),
// por eso no se tipa como JSON: se arma un FormData en el servicio a partir de esto.
export interface ProductFormValue {
  name: string;
  description: string;
  price: number;
  sku: string;
  stock: number;
  categoryId: number;
  image: File | null;
}
