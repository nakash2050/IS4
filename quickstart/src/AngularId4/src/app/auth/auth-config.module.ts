import { APP_INITIALIZER, NgModule } from '@angular/core';
import { AuthModule, LogLevel, OidcConfigService } from 'angular-auth-oidc-client';
import { environment } from '@env/environment';

export function configureAuth(oidcConfigService: OidcConfigService): () => Promise<any> {
  return () =>
    oidcConfigService.withConfig({
      stsServer: environment.stsServer,
      redirectUrl: window.location.origin,
      postLogoutRedirectUri: window.location.origin,
      clientId: environment.clientId,
      scope: environment.scope, //'openid profile email roles app.api.employeeprofile.read'
      responseType: 'code',
      silentRenew: true,
      silentRenewUrl: `${window.location.origin}/silent-renew.html`,
      useRefreshToken: false,
      postLoginRoute: window.location.origin,
      renewTimeBeforeTokenExpiresInSeconds: 30,
      logLevel: LogLevel.Debug,
      secureRoutes: environment.secureRoutes, //['https://localhost:44321/api', 'https://devkit-api-employeeprofile.azurewebsites.net/api'],
    });
}

@NgModule({
  imports: [AuthModule.forRoot()],
  exports: [AuthModule],
  providers: [
    OidcConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: configureAuth,
      deps: [OidcConfigService],
      multi: true,
    },
  ],
})
export class AuthConfigModule {}
