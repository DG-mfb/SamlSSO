"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var login_component_1 = require("./login/login.component");
exports.AppRoutes = [
    {
        path: '',
        redirectTo: '/sso/login',
        pathMatch: 'full'
    },
    { path: 'sso/login', component: login_component_1.LoginComponent, data: { title: 'Login' } },
    { path: '**', component: login_component_1.LoginComponent, data: { title: 'Login' } },
];
//# sourceMappingURL=app.routing.js.map