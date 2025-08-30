import { Component, inject } from '@angular/core';
import { Todo } from '../../types/api/todo.model';
import { TodoService } from '../todos/todo';
import { Observable, Subscription, pipe, startWith, switchMap } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { Draggable } from '../draggable/draggable';
import {
  CdkDragDrop,
  CdkDropList,
  moveItemInArray,
} from '@angular/cdk/drag-drop';
import { AddButton } from '../add-button/add-button';

@Component({
  selector: 'app-home',
  imports: [AsyncPipe, Draggable, CdkDropList, AddButton],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  private todoService: TodoService = inject(TodoService);
  public todos$: Observable<Todo[]>;

  constructor() {
    this.todos$ = this.todoService.refresh$.pipe(
      startWith(undefined),
      switchMap(() => this.todoService.fetchTodos())
    );
  }

  public drop(event: CdkDragDrop<Todo[]>) {
    this.todos$.subscribe((todos) => {
      moveItemInArray(todos, event.previousIndex, event.currentIndex);
    });
  }
}
