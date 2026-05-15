import { mergeApplicationConfig, ApplicationConfig } from '@angular/core';
import { provideServerRendering, withRoutes } from '@angular/ssr';
import { provideImgixLoader } from '@angular/common';
import { appConfig } from './app.config';
import { serverRoutes } from './app.routes.server';

const serverConfig: ApplicationConfig = {
  providers: [
    provideServerRendering(withRoutes(serverRoutes)),
    provideImgixLoader('https://assets.imgix.net/')
  ]
};

export const config = mergeApplicationConfig(appConfig, serverConfig);
