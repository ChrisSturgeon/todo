import { TestBed } from '@angular/core/testing';

import { TodoService } from './todo.service';
import { HttpClient } from '@angular/common/http';

describe('Todo', () => {
  let todoService: TodoService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    todoService = TestBed.inject(TodoService);
  });

  it('should be created', () => {
    expect(todoService).toBeTruthy();
  });
});
