import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { ArticlesList } from "./components/articles.list/articles.list.component"
import { ArticleView } from "./components/article.view/article.view.component"
import { CommentView } from "./components/comment.view/comment.view.component"

import { ViewDataService } from "./services/viewdata.service";

@NgModule({
  declarations: [
    AppComponent,
    ArticlesList,
    ArticleView,
    CommentView
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule
  ],
  providers: [ViewDataService],
  bootstrap: [AppComponent]
})
export class AppModule { }
