import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CategoryService } from '../../../../core/services/category.service';
import { ProductService } from '../../../../core/services/product.service';
import { Category } from '../../../../core/models/category.model';

@Component({
  selector: 'app-product-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './product-form.html',
})
export class ProductForm implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
    description: [''],
    price: [0, [Validators.required, Validators.min(0.01)]],
    sku: ['', Validators.required],
    stock: [0, [Validators.required, Validators.min(0)]],
    categoryId: [0, [Validators.required, Validators.min(1)]],
  });

  readonly categories = signal<Category[]>([]);
  readonly selectedImage = signal<File | null>(null);
  readonly currentImageUrl = signal<string | null>(null);
  readonly productId = signal<number | null>(null);
  readonly errorMessage = signal<string | null>(null);
  readonly isSubmitting = signal(false);

  get isEditMode(): boolean {
    return this.productId() !== null;
  }

  ngOnInit(): void {
    this.categoryService.getCategories().subscribe({ next: (categories) => this.categories.set(categories) });

    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    const id = Number(idParam);
    this.productId.set(id);
    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.form.patchValue({
          name: product.name,
          description: product.description,
          price: product.price,
          sku: product.sku,
          stock: product.stock,
          categoryId: product.categoryId,
        });
        this.currentImageUrl.set(product.imgUrl);
      },
      error: () => this.errorMessage.set('No se pudo cargar el producto.'),
    });
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedImage.set(input.files?.[0] ?? null);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMessage.set(null);
    this.isSubmitting.set(true);
    const formValue = { ...this.form.getRawValue(), image: this.selectedImage() };

    const request$ = this.isEditMode
      ? this.productService.updateProduct(this.productId()!, formValue)
      : this.productService.createProduct(formValue);

    request$.subscribe({
      next: () => this.router.navigate(['/admin/products']),
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('No se pudo guardar el producto. Verifica los datos.');
      },
    });
  }
}
