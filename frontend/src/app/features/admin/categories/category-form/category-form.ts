import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CategoryService } from '../../../../core/services/category.service';

@Component({
  selector: 'app-category-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './category-form.html',
})
export class CategoryForm implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
  });

  readonly categoryId = signal<number | null>(null);
  readonly errorMessage = signal<string | null>(null);
  readonly isSubmitting = signal(false);

  get isEditMode(): boolean {
    return this.categoryId() !== null;
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    const id = Number(idParam);
    this.categoryId.set(id);
    this.categoryService.getCategoryById(id).subscribe({
      next: (category) => this.form.patchValue({ name: category.name }),
      error: () => this.errorMessage.set('No se pudo cargar la categoría.'),
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMessage.set(null);
    this.isSubmitting.set(true);
    const request$ = this.isEditMode
      ? this.categoryService.updateCategory(this.categoryId()!, this.form.getRawValue())
      : this.categoryService.createCategory(this.form.getRawValue());

    request$.subscribe({
      next: () => this.router.navigate(['/admin/categories']),
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('No se pudo guardar la categoría (¿ya existe una con ese nombre?).');
      },
    });
  }
}
