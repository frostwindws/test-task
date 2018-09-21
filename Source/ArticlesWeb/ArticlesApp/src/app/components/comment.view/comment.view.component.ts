import { Component, OnInit, Input } from "@angular/core";

import { ArticleComment } from "../../models/article.comment";

/**
 * Компонент отображения комментария.
 */
@Component({
  selector: "comment-view",
  templateUrl: "./comment.view.component.html",
  styleUrls: ["./comment.view.component.less"]
})
export class CommentView implements OnInit {
  @Input() comment = ArticleComment;

  /**
   * Конструктор компонента.
   */
  constructor() { }

  /**
   * Инициализация компонента.
   */
  ngOnInit() {
  }

  /**
   * Запуск редактирования комментария.
   */
  onEdit() {
    
  }

  /**
   * Запрос удаления комментария.
   */
  onDelete() {

  }
}

