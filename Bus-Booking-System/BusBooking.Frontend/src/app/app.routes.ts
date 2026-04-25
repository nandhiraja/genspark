import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/user/search', pathMatch: 'full' },

  // Auth
  { path: 'auth/login', loadComponent: () => import('./modules/auth/login').then(m => m.LoginComponent) },
  { path: 'auth/register', loadComponent: () => import('./modules/auth/register').then(m => m.RegisterComponent) },

  // User
  { path: 'user/search', loadComponent: () => import('./modules/user/search').then(m => m.SearchComponent) },
  { path: 'user/seats/:busId', canActivate: [authGuard], data: { role: 'USER' }, loadComponent: () => import('./modules/user/seats').then(m => m.SeatsComponent) },
  { path: 'user/payment', canActivate: [authGuard], data: { role: 'USER' }, loadComponent: () => import('./modules/user/payment').then(m => m.PaymentComponent) },
  { path: 'user/dashboard', canActivate: [authGuard], data: { role: 'USER' }, loadComponent: () => import('./modules/user/dashboard').then(m => m.DashboardComponent) },
  { path: 'user/profile', canActivate: [authGuard], data: { role: 'USER' }, loadComponent: () => import('./modules/user/profile').then(m => m.ProfileComponent) },
  { path: 'user/apply-operator', canActivate: [authGuard], data: { role: 'USER' }, loadComponent: () => import('./modules/user/apply-operator').then(m => m.ApplyOperatorComponent) },

  // Operator
  { path: 'operator/dashboard', canActivate: [authGuard], data: { role: 'OPERATOR' }, loadComponent: () => import('./modules/operator/operator').then(m => m.OpDashboardComponent) },
  { path: 'operator/add-bus', canActivate: [authGuard], data: { role: 'OPERATOR' }, loadComponent: () => import('./modules/operator/operator').then(m => m.AddBusComponent) },
  { path: 'operator/manage', canActivate: [authGuard], data: { role: 'OPERATOR' }, loadComponent: () => import('./modules/operator/operator').then(m => m.ManageBusesComponent) },
  { path: 'operator/boarding-points', canActivate: [authGuard], data: { role: 'OPERATOR' }, loadComponent: () => import('./modules/operator/operator').then(m => m.OperatorBoardingPointsComponent) },

  // Admin
  { path: 'admin/dashboard', canActivate: [authGuard], data: { role: 'ADMIN' }, loadComponent: () => import('./modules/admin/admin').then(m => m.AdminDashboardComponent) },
  { path: 'admin/requests/buses', canActivate: [authGuard], data: { role: 'ADMIN' }, loadComponent: () => import('./modules/admin/admin').then(m => m.AdminBusRequestsComponent) },
  { path: 'admin/requests/operators', canActivate: [authGuard], data: { role: 'ADMIN' }, loadComponent: () => import('./modules/admin/admin').then(m => m.AdminOperatorRequestsComponent) },
  { path: 'admin/operators', canActivate: [authGuard], data: { role: 'ADMIN' }, loadComponent: () => import('./modules/admin/admin').then(m => m.AdminOperatorsComponent) },
  { path: 'admin/operators/:operatorId', canActivate: [authGuard], data: { role: 'ADMIN' }, loadComponent: () => import('./modules/admin/admin').then(m => m.AdminOperatorDetailComponent) },
  { path: 'admin/routes', canActivate: [authGuard], data: { role: 'ADMIN' }, loadComponent: () => import('./modules/admin/admin').then(m => m.AdminRoutesComponent) },
  { path: 'admin/requests/route-changes', canActivate: [authGuard], data: { role: 'ADMIN' }, loadComponent: () => import('./modules/admin/admin').then(m => m.AdminRouteChangeRequestsComponent) },
  { path: 'admin/requests/reactivations', canActivate: [authGuard], data: { role: 'ADMIN' }, loadComponent: () => import('./modules/admin/admin').then(m => m.AdminReactivationRequestsComponent) },

  // 404
  { path: '**', redirectTo: '/user/search' }
];
