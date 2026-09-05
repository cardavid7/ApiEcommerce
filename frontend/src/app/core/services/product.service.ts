import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PaginatedResponse, PaginationParams } from '../models/pagination.model';
import { Product, ProductFormValue } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly apiUrl = `${environment.apiUrl}/v1/Products`;

  constructor(private readonly http: HttpClient) {}

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  getProductsPaginated(params: PaginationParams): Observable<PaginatedResponse<Product>> {
    return this.http.get<PaginatedResponse<Product>>(`${this.apiUrl}/Paginated`, {
      params: { pageNumber: params.pageNumber, pageSize: params.pageSize },
    });
  }

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  getProductsForCategory(categoryId: number): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/SearchByCategory/${categoryId}`);
  }

  searchProducts(term: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/SearchByNameOrDescription/${encodeURIComponent(term)}`);
  }

  createProduct(formValue: ProductFormValue): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, this.toFormData(formValue));
  }

  updateProduct(id: number, formValue: ProductFormValue): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/${id}`, this.toFormData(formValue));
  }

  deleteProduct(id: number): Observable<string> {
    return this.http.delete(`${this.apiUrl}/${id}`, { responseType: 'text' });
  }

  buyProduct(name: string, quantity: number): Observable<string> {
    return this.http.patch(`${this.apiUrl}/BuyProduct/${encodeURIComponent(name)}/${quantity}`, null, {
      responseType: 'text',
    });
  }

  private toFormData(formValue: ProductFormValue): FormData {
    const formData = new FormData();
    formData.append('Name', formValue.name);
    formData.append('Description', formValue.description);
    formData.append('Price', String(formValue.price));
    formData.append('SKU', formValue.sku);
    formData.append('Stock', String(formValue.stock));
    formData.append('CategoryId', String(formValue.categoryId));
    if (formValue.image) {
      formData.append('Image', formValue.image);
    }
    return formData;
  }
}
