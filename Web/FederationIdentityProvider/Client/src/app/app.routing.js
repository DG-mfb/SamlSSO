"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var login_component_1 = require("./login/login.component");
var HomeComponent_1 = require("./Home/HomeComponent");
var AuthGuard_1 = require("./Guards/AuthGuard");
exports.AppRoutes = [
    //{
    //	path: '',
    //	//component: LoginComponent
    //	redirectTo: '/login',
    //	pathMatch: 'full'
    //},
    { path: '', component: HomeComponent_1.HomeComponent, canActivate: [AuthGuard_1.AuthGuard] },
    { path: 'login', component: login_component_1.LoginComponent, data: { title: 'Login' } },
    { path: '**', redirectTo: '' },
];
//# sourceMappingURL=app.routing.js.map