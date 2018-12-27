import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';
import { NotFoundComponent } from './shared';
import { AuthGuard, LandingGuard } from './core';

const routes: Routes = [
  {
    path: '',
    loadChildren: './modules/landing/landing.module#LandingModule',
    canActivate: [LandingGuard]
  },
  {
    path: 'play',
    loadChildren: './modules/chess-game/chess-game.module#ChessGameModule',
    canActivate: [AuthGuard]
  },
  {
    path: 'puzzles',
    loadChildren: './modules/puzzles/puzzles.module#PuzzlesModule',
    canActivate: [AuthGuard]
  },
  {
    path: 'rules',
    loadChildren: './modules/rules/rules.module#RulesModule',
    canActivate: [AuthGuard]
  },
  { path: '404', component: NotFoundComponent },
  { path: '**', redirectTo: '/404' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    // preload all modules; optionally we could
    // implement a custom preloading strategy for just some
    // of the modules (PRs welcome 😉)
    preloadingStrategy: PreloadAllModules
  })],
  exports: [RouterModule],
  providers: [AuthGuard,LandingGuard]
})
export class AppRoutingModule { }
