import { TestBed } from '@angular/core/testing';
import { TodoService } from './todo.service';
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { environment } from '../../environments/environment';
import { TodoResponse } from '../../../api-types/api.types';
import { switchMap } from 'rxjs';
import type { TodosResponse, TodoPosition } from '../../../api-types/api.types';

describe('Todo', () => {
  let todoService: TodoService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiUrl}/todo`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TodoService, provideHttpClient(), provideHttpClientTesting()],
    });

    todoService = TestBed.inject(TodoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(todoService).toBeTruthy();
  });

  describe('fetchTodos', () => {
    it('should fetch todos', () => {
      const mockResponse: TodosResponse = {
        items: [
          {
            id: '12345',
            name: 'Walk the dog',
            description: 'Take the dog for a walk',
            position: 0,
            completed: false,
            createdAt: 'createdAt',
            updatedAt: 'updatedAt',
          },
        ],
        totalCount: 0,
      };

      todoService.fetchTodos().subscribe((response) => {
        expect(response).toEqual(mockResponse);
      });

      const request = httpMock.expectOne(baseUrl);
      expect(request.request.method).toBe('GET');
      request.flush(mockResponse);
    });

    it('should handle an empty todos reponse', () => {
      const mockResponse: TodosResponse = {
        items: [],
        totalCount: 0,
      };

      todoService.fetchTodos().subscribe((response) => {
        expect(response).toEqual(mockResponse);
      });

      const req = httpMock.expectOne(baseUrl);
      req.flush(mockResponse);
    });

    it('should propogate an error from the API', () => {
      const mockError = { status: 500, statusText: 'Internal server error' };

      todoService.fetchTodos().subscribe({
        next: () => fail('An error occurred'),
        error: (err) => {
          expect(err.status).toBe(500);
        },
      });

      const req = httpMock.expectOne(baseUrl);
      req.flush('Something went wrong', mockError);
    });
  });

  describe('createTodo', () => {
    it('should make a POST request', () => {
      const todoName = 'Walk the dog';

      todoService.createTodo(todoName).subscribe();

      const req = httpMock.expectOne(baseUrl);
      expect(req.request.method).toBe('POST');
    });

    it('should send the correct request body', () => {
      const todoName = 'Walk the dog';

      todoService.createTodo(todoName).subscribe();

      const req = httpMock.expectOne(baseUrl);
      expect(req.request.body).toEqual({ name: 'Walk the dog' });
    });

    it('should create a new todo and return the new todo', () => {
      const mockResponse: TodoResponse = {
        id: '23556',
        name: 'Spring clean',
        position: 1,
        completed: false,
        createdAt: 'createdAt',
        updatedAt: 'updatedAt',
      };

      todoService.createTodo('Spring clean').subscribe((response) => {
        expect(response).toEqual(mockResponse);
      });

      const request = httpMock.expectOne(baseUrl);
      expect(request.request.method).toBe('POST');
      expect(request.request.body).toEqual({ name: 'Spring clean' });
      request.flush(mockResponse);
    });

    it('should propogate errors from the API', () => {
      const todoName = 'Spring clean';
      const mockError = { status: 500, statusText: 'Internal server error' };

      todoService.createTodo(todoName).subscribe({
        next: () => fail('Expected an error, not a todo'),
        error: (err) => {
          expect(err.status).toBe(500);
        },
      });

      const req = httpMock.expectOne(baseUrl);
      req.flush('Something went wrong', mockError);
    });
  });

  describe('updateTodo', () => {
    const todoId = '1234';
    const updatedTodo: Partial<TodoResponse> = {
      completed: true,
    };

    it('should make a PATCH request', () => {
      todoService.updateTodo(todoId, updatedTodo).subscribe();

      const req = httpMock.expectOne(`${baseUrl}/${todoId}`);

      expect(req.request.method).toBe('PATCH');
      req.flush(null);
    });

    it('should include the updated todo in the request body', () => {
      todoService.updateTodo(todoId, updatedTodo).subscribe();

      const req = httpMock.expectOne(`${baseUrl}/${todoId}`);

      expect(req.request.body).toEqual(updatedTodo);
    });

    it('should propogate errors from the API', () => {
      const mockError = { status: 500, statusText: 'Internal server error' };

      todoService.updateTodo(todoId, updatedTodo).subscribe({
        next: () => fail('Expected an error'),
        error: (err) => {
          expect(err.status).toBe(500);
        },
      });

      const req = httpMock.expectOne(`${baseUrl}/${todoId}`);

      req.flush('Something went wrong', mockError);
    });
  });

  describe('deleteTodo', () => {
    const todoId = '1234';

    it('make an HTTP DELETE request', () => {
      todoService.deleteTodo(todoId).subscribe();

      const req = httpMock.expectOne(`${baseUrl}/${todoId}`);
      expect(req.request.method).toBe('DELETE');

      req.flush(null);
    });

    it('should propogate errors from the API', () => {
      const mockError = { status: 500, statusText: 'Internal server error' };

      todoService.deleteTodo(todoId).subscribe({
        next: () => fail('Expected an error'),
        error: (err: HttpErrorResponse) => {
          expect(err.status).toBe(mockError.status);
          expect(err.statusText).toBe(mockError.statusText);
        },
      });

      const req = httpMock.expectOne(`${baseUrl}/${todoId}`);
      req.flush('Something went wrong', mockError);
    });
  });

  describe('reorderTodos', () => {
    const reorderEndpoint = `${baseUrl}/reorder`;
    const todos: TodoResponse[] = [
      {
        id: '1',
        name: 'Spring clean',
        position: 1,
        completed: false,
        createdAt: 'createdAt',
        updatedAt: 'updatedAt',
      },
      {
        id: '2',
        name: 'Walk the dog',
        position: 0,
        completed: true,
        createdAt: 'createdAt',
        updatedAt: 'updatedAt',
      },
    ];

    it('should make an HTTP put request', () => {
      todoService.reorderTodos(todos).subscribe();

      const req = httpMock.expectOne(reorderEndpoint);

      expect(req.request.method).toBe('PUT');
    });

    it('should propogate errors from the API', () => {
      var mockError = { status: 500, statusText: 'Internal server error' };

      todoService.reorderTodos(todos).subscribe({
        next: () => fail('Expect failure'),
        error: (err: HttpErrorResponse) => {
          expect(err.status).toBe(500);
          expect(err.statusText).toBe('Internal server error');
        },
      });

      var req = httpMock.expectOne(reorderEndpoint);

      req.flush('Something went wrong', mockError);
    });
  });

  describe('refresh behaviour', () => {
    const mockResponse: TodosResponse = { items: [], totalCount: 0 };

    it('should emit initial value on subscription', (done) => {
      todoService.refresh$
        .pipe(switchMap(() => todoService.fetchTodos()))
        .subscribe((value) => {
          expect(value).toEqual(mockResponse);
          done();
        });

      const req = httpMock.expectOne(baseUrl);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });

    it('should trigger a new emission when triggerRefresh is called', () => {
      const responses: TodosResponse[] = [];

      todoService.refresh$
        .pipe(switchMap(() => todoService.fetchTodos()))
        .subscribe((response) => responses.push(response));

      const initialReq = httpMock.expectOne(baseUrl);
      expect(initialReq.request.method).toBe('GET');
      initialReq.flush(mockResponse);

      todoService.triggerRefresh();

      const refreshReq = httpMock.expectOne(baseUrl);
      expect(refreshReq.request.method).toBe('GET');
      refreshReq.flush(mockResponse);

      expect(responses.length).toBe(2);
      expect(responses[0]).toEqual(mockResponse);
      expect(responses[1]).toEqual(mockResponse);
    });

    it('should propogate HTTP errors on refresh', () => {
      const errorResponse = {
        status: 500,
        statusText: 'Internal server error',
      };
      let errorReceived: any;

      todoService.refresh$
        .pipe(switchMap(() => todoService.fetchTodos()))
        .subscribe({
          next: () => {},
          error: (err) => {
            errorReceived = err;
          },
        });

      const req = httpMock.expectOne(baseUrl);
      req.flush('Something went wrong', errorResponse);

      expect(errorReceived.status).toBe(500);
      expect(errorReceived.statusText).toBe('Internal server error');
    });
  });
});
