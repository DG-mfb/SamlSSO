"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var AuthenticationService_1 = require("../services/AuthenticationService");
var LoginComponent = /** @class */ (function () {
    function LoginComponent(route, router, authenticationService) {
        this.route = route;
        this.router = router;
        this.authenticationService = authenticationService;
        this.showWarningMessage = false;
        this.showErrorMessage = false;
        this.loading = false;
        this.model = {};
    }
    LoginComponent.prototype.ngOnInit = function () {
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    };
    LoginComponent.prototype.login = function () {
        var _this = this;
        this.clearMessages();
        var req = this.authenticationService.login(this.model.username, this.model.password);
        req.subscribe(function (data) {
            _this.router.navigate([_this.returnUrl]);
        }, function (error) {
            //this.alertService.error(error);
            _this.loading = false;
        });
        //this.userService.currentUser$
        //    .subscribe(user => {
        //        if (user != null) {
        //            this.clearMessages();
        //            this.router.navigate(['/']);   
        //        }
        //        else {
        //            this.showErrorMessage = false;
        //            this.showWarningMessage = true;
        //        }                    
        //    },
        //    error => {
        //        this.showErrorMessage = true;
        //        this.showWarningMessage = false;
        //    });
    };
    LoginComponent.prototype.clearMessages = function () {
        this.showWarningMessage = false;
        this.showErrorMessage = false;
    };
    LoginComponent = __decorate([
        core_1.Component({
            moduleId: module.id,
            selector: 'login',
            templateUrl: 'login.component.html',
            providers: [AuthenticationService_1.AuthenticationService],
            styleUrls: ['login.component.css']
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof router_1.ActivatedRoute !== "undefined" && router_1.ActivatedRoute) === "function" && _a || Object, typeof (_b = typeof router_1.Router !== "undefined" && router_1.Router) === "function" && _b || Object, AuthenticationService_1.AuthenticationService])
    ], LoginComponent);
    return LoginComponent;
    var _a, _b;
}());
exports.LoginComponent = LoginComponent;
//# sourceMappingURL=login.component.js.map