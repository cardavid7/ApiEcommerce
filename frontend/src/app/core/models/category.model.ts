export interface Category {
  id: number;
  name: string;
  creationDate: string;
}

export interface CreateCategoryRequest {
  name: string;
}
