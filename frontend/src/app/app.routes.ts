import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { roleGuard } from './core/guards/role.guard';
import { CategoryForm } from './features/admin/categories/category-form/category-form';
import { CategoryList } from './features/admin/categories/category-list/category-list';
import { ProductForm } from './features/admin/products/product-form/product-form';
import { ProductListAdmin } from './features/admin/products/product-list-admin/product-list-admin';
import { UserList } from './features/admin/users/user-list/user-list';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { ProductDetail } from './features/catalog/product-detail/product-detail';
import { ProductList } from './features/catalog/product-list/product-list';
import { Profile } from './features/profile/profile';
import { AdminLayout } from './layout/admin-layout/admin-layout';
import { PublicLayout } from './layout/public-layout/public-layout';
import { Forbidden } from './shared/forbidden/forbidden';
import { NotFound } from './shared/not-found/not-found';

export const routes: Routes = [
  {
    path: '',
    component: PublicLayout,
    children: [
      { path: '', component: ProductList },
      { path: 'products/:id', component: ProductDetail },
      { path: 'login', component: Login, canActivate: [guestGuard] },
      { path: 'register', component: Register, canActivate: [guestGuard] },
      { path: 'profile', component: Profile, canActivate: [authGuard] },
      { path: 'forbidden', component: Forbidden },
    ],
  },
  {
    path: 'admin',
    component: AdminLayout,
    canActivate: [roleGuard(['Admin'])],
    children: [
      { path: '', redirectTo: 'products', pathMatch: 'full' },
      { path: 'products', component: ProductListAdmin },
      { path: 'products/new', component: ProductForm },
      { path: 'products/:id/edit', component: ProductForm },
      { path: 'categories', component: CategoryList },
      { path: 'categories/new', component: CategoryForm },
      { path: 'categories/:id/edit', component: CategoryForm },
      { path: 'users', component: UserList },
    ],
  },
  { path: '**', component: NotFound },
];
