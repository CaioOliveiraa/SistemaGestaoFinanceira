import {
    HttpRequest,
    HttpHandlerFn,
    HttpInterceptorFn,
} from '@angular/common/http';

export const apiInterceptor: HttpInterceptorFn = (
    req: HttpRequest<any>,
    next: HttpHandlerFn
) => {
    const authReq = req.clone({ withCredentials: true });
    return next(authReq);
};
