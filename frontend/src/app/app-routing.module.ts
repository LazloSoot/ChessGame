import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';
import { NotFoundComponent } from './shared';

const routes: Routes = [
  {
    path: '',
    loadChildren: './modules/home/home.module#HomeModule'
  },
  {
    path: 'play',
    loadChildren: './modules/chess-game/chess-game.module#ChessGameModule'
  },
  {
    path: 'puzzles',
    loadChildren: './modules/puzzles/puzzles.module#PuzzlesModule'
  },
  {
    path: 'rules',
    loadChildren: './modules/rules/rules.module#RulesModule'
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
  exports: [RouterModule]
})
export class AppRoutingModule { }
