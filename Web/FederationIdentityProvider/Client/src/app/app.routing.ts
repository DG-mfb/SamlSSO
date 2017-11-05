import { LoginComponent } from './login/login.component';
import { AppComponent } from "./app.component";
export const AppRoutes = [
	{
		path: '',
		component: LoginComponent
		//redirectTo: 'auth/sso/login',
		//pathMatch: 'full'
	},
	{ path: 'auth/sso/login', component: LoginComponent, data: { title: 'Login' } },
	{ path: '**', redirectTo: '' },
];