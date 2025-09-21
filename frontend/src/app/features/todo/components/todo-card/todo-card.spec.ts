import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { TodoCard } from './todo-card';
import { TodoService } from '../../services/todo-service/todo.service';
import { TodoResponse } from '@api-types/todo/todo.types';

describe('Draggable', () => {
  let component: TodoCard;
  let fixture: ComponentFixture<TodoCard>;
  let mockTodoService: jasmine.SpyObj<TodoService>;

  const mockTodo: TodoResponse = {
    id: '1',
    name: 'Test todo',
    description: 'Just a test',
    position: 0,
    completed: false,
    createdAt: 'now',
    updatedAt: 'now',
  };

  beforeEach(async () => {
    mockTodoService = jasmine.createSpyObj('TodoService', ['updateTodo']);

    await TestBed.configureTestingModule({
      imports: [TodoCard],
      providers: [{ provide: TodoService, useValue: mockTodoService }],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoCard);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('markup', () => {
    it('should render the todo name from @Input', () => {
      fixture.componentRef.setInput('todo', mockTodo);
      fixture.detectChanges();
      expect(component.todo().name).toBe('Test todo');
    });
  });

  describe('toggling completed behaviour', () => {
    it('should toggle completed and call updateTodo', () => {
      fixture.componentRef.setInput('todo', { ...mockTodo });
      fixture.detectChanges();
      mockTodoService.updateTodo.and.returnValue(of(void 0));

      component.toggleDone();

      expect(component.todo().completed).toBeTrue();
      expect(mockTodoService.updateTodo).toHaveBeenCalledWith(
        mockTodo.id,
        jasmine.objectContaining({ completed: true })
      );
    });

    it('should revert completed if updateToDo fails', () => {
      fixture.componentRef.setInput('todo', { ...mockTodo });

      mockTodoService.updateTodo.and.returnValue(
        throwError(() => new Error('fail!'))
      );

      component.toggleDone();

      expect(component.todo().completed).toBeFalse();
    });
  });
});
