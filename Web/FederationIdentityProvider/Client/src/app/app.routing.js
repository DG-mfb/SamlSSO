"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var login_component_1 = require("./login/login.component");
exports.AppRoutes = [
    {
        path: '',
        component: login_component_1.LoginComponent
        //redirectTo: 'auth/sso/login',
        //pathMatch: 'full'
    },
    { path: 'auth/sso/login', component: login_component_1.LoginComponent, data: { title: 'Login' } },
    { path: '**', redirectTo: '' },
];
//# sourceMappingURL=app.routing.js.map