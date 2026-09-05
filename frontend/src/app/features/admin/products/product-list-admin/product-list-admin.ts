import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../../../core/services/product.service';
import { Product } from '../../../../core/models/product.model';

@Component({
  selector: 'app-product-list-admin',
  imports: [RouterLink, CurrencyPipe],
  templateUrl: './product-list-admin.html',
})
export class ProductListAdmin implements OnInit {
  readonly products = signal<Product[]>([]);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  constructor(private readonly productService: ProductService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
    this.productService.getProducts().subscribe({
      next: (products) => {
        this.products.set(products);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar los productos.');
        this.isLoading.set(false);
      },
    });
  }

  remove(product: Product): void {
    if (!confirm(`¿Eliminar el producto "${product.name}"?`)) {
      return;
    }
    this.productService.deleteProduct(product.id).subscribe({
      next: () => this.load(),
      error: () => this.errorMessage.set('No se pudo eliminar el producto.'),
    });
  }
}
