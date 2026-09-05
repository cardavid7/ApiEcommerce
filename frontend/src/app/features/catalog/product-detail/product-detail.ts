import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../core/models/product.model';

@Component({
  selector: 'app-product-detail',
  imports: [FormsModule, RouterLink, CurrencyPipe],
  templateUrl: './product-detail.html',
})
export class ProductDetail implements OnInit {
  readonly product = signal<Product | null>(null);
  readonly isLoading = signal(true);
  readonly notFound = signal(false);

  readonly quantity = signal(1);
  readonly purchaseMessage = signal<string | null>(null);
  readonly purchaseError = signal<string | null>(null);
  readonly isPurchasing = signal(false);

  constructor(
    private readonly route: ActivatedRoute,
    private readonly productService: ProductService,
    readonly authService: AuthService,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.product.set(product);
        this.isLoading.set(false);
      },
      error: () => {
        this.notFound.set(true);
        this.isLoading.set(false);
      },
    });
  }

  buy(): void {
    const product = this.product();
    if (!product || this.quantity() < 1) {
      return;
    }

    this.purchaseMessage.set(null);
    this.purchaseError.set(null);
    this.isPurchasing.set(true);

    this.productService.buyProduct(product.name, this.quantity()).subscribe({
      next: (message) => {
        this.purchaseMessage.set(message);
        this.isPurchasing.set(false);
        this.product.set({ ...product, stock: product.stock - this.quantity() });
      },
      error: (error) => {
        this.purchaseError.set(typeof error?.error === 'string' ? error.error : 'No se pudo completar la compra.');
        this.isPurchasing.set(false);
      },
    });
  }
}
