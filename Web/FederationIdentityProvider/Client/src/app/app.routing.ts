import { LoginComponent } from './login/login.component';
export const AppRoutes = [
	{
		path: '',
		redirectTo: '/sso/login',
		pathMatch: 'full'
	},
	{ path: 'sso/login', component: LoginComponent, data: { title: 'Login' } },
	{ path: '**', component: LoginComponent, data: { title: 'Login' } },
];