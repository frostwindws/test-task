import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule, MatIconModule, MatTableModule, MatAutocompleteModule, MatInputModule, MatDialogModule, MatSnackBarModule } from '@angular/material';

import { AppComponent } from './app.component';
import { ArticlesList } from "./components/articles.list/articles.list.component"
import { ArticleView } from "./components/article.view/article.view.component"
import { CommentView } from "./components/comment.view/comment.view.component"
import { ArticleEditor } from './components/article.editor/article.editor.component';
import { CommentEditor } from './components/comment.editor/comment.editor.component';
import { Prompt } from './components/prompt/prompt.component';

import { ViewDataService } from "./services/viewdata.service";

@NgModule({
  declarations: [
    AppComponent,
    ArticlesList,
    ArticleView,
    CommentView,
    ArticleEditor,
    CommentEditor,
    Prompt
  ],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatAutocompleteModule,
    MatInputModule,
    MatDialogModule,
    MatSnackBarModule
  ],
  entryComponents: [ArticleEditor, CommentEditor, Prompt],
  providers: [ViewDataService],
  bootstrap: [AppComponent]
})
export class AppModule { }
