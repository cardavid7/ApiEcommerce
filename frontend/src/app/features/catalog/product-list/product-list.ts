import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../../../core/services/category.service';
import { ProductService } from '../../../core/services/product.service';
import { Category } from '../../../core/models/category.model';
import { Product } from '../../../core/models/product.model';

type ViewMode = 'all' | 'search' | 'category';

@Component({
  selector: 'app-product-list',
  imports: [FormsModule, RouterLink, CurrencyPipe],
  templateUrl: './product-list.html',
})
export class ProductList implements OnInit {
  readonly products = signal<Product[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly searchTerm = signal('');
  readonly selectedCategoryId = signal<number | null>(null);
  readonly mode = signal<ViewMode>('all');

  readonly pageNumber = signal(1);
  readonly pageSize = 8;
  readonly totalPages = signal(1);

  constructor(
    private readonly productService: ProductService,
    private readonly categoryService: CategoryService,
  ) {}

  ngOnInit(): void {
    this.categoryService.getCategories().subscribe({ next: (categories) => this.categories.set(categories) });
    this.loadPaginated();
  }

  loadPaginated(): void {
    this.mode.set('all');
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.productService.getProductsPaginated({ pageNumber: this.pageNumber(), pageSize: this.pageSize }).subscribe({
      next: (response) => {
        this.products.set(response.items);
        this.totalPages.set(response.totalPages);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar los productos.');
        this.isLoading.set(false);
      },
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.pageNumber.set(page);
    this.loadPaginated();
  }

  search(): void {
    const term = this.searchTerm().trim();
    if (!term) {
      this.pageNumber.set(1);
      this.loadPaginated();
      return;
    }

    this.selectedCategoryId.set(null);
    this.mode.set('search');
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.productService.searchProducts(term).subscribe({
      next: (products) => {
        this.products.set(products);
        this.isLoading.set(false);
      },
      error: () => {
        this.products.set([]);
        this.isLoading.set(false);
      },
    });
  }

  filterByCategory(categoryId: string): void {
    const id = Number(categoryId);
    if (!id) {
      this.selectedCategoryId.set(null);
      this.searchTerm.set('');
      this.pageNumber.set(1);
      this.loadPaginated();
      return;
    }

    this.selectedCategoryId.set(id);
    this.searchTerm.set('');
    this.mode.set('category');
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.productService.getProductsForCategory(id).subscribe({
      next: (products) => {
        this.products.set(products);
        this.isLoading.set(false);
      },
      error: () => {
        this.products.set([]);
        this.isLoading.set(false);
      },
    });
  }
}
