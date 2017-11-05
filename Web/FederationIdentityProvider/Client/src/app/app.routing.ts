import { LoginComponent } from './login/login.component';
import { AppComponent } from "./app.component";
import { HomeComponent } from "./Home/HomeComponent";
import { AuthGuard } from "./Guards/AuthGuard";
export const AppRoutes = [
	//{
	//	path: '',
	//	//component: LoginComponent
	//	redirectTo: '/login',
	//	pathMatch: 'full'
	//},
	{path: '', component: HomeComponent, canActivate: [AuthGuard] },
	{ path: 'login', component: LoginComponent, data: { title: 'Login' } },
	{ path: '**', redirectTo: '' },
];