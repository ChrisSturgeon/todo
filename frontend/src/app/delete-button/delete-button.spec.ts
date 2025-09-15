import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DeleteButton } from './delete-button';
import { TodoService } from '../todo-service/todo.service';
import { of, throwError } from 'rxjs';

describe('DeleteButton', () => {
  let component: DeleteButton;
  let fixture: ComponentFixture<DeleteButton>;
  let mockTodoService: jasmine.SpyObj<TodoService>;

  const todoId = 'f36b0f79-ff17-4888-b596-36adff38eb98';

  beforeEach(async () => {
    mockTodoService = jasmine.createSpyObj('TodoService', [
      'deleteTodo',
      'triggerRefresh',
    ]);

    await TestBed.configureTestingModule({
      imports: [DeleteButton],
      providers: [{ provide: TodoService, useValue: mockTodoService }],
    }).compileComponents();

    fixture = TestBed.createComponent(DeleteButton);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('markup', () => {
    it('should have a button labelled "Delete"', () => {
      fixture.componentRef.setInput('todoId', todoId);
      const button: HTMLButtonElement =
        fixture.nativeElement.querySelector('button');

      expect(button).not.toBeNull();
      expect(button.textContent).toBe('Delete');
    });
  });

  describe('delete behaviour', () => {
    it('should call deleteTodo with todo id and trigger refresh on success', () => {
      fixture.componentRef.setInput('todoId', todoId);
      fixture.detectChanges();
      mockTodoService.deleteTodo.and.returnValue(of(void 0));

      component.deleteTodo();

      expect(mockTodoService.deleteTodo).toHaveBeenCalledWith(todoId);
      expect(mockTodoService.triggerRefresh).toHaveBeenCalled();
    });

    it('should not call triggerRefresh if deleteTodo fails', () => {
      fixture.componentRef.setInput('todoId', todoId);

      mockTodoService.deleteTodo.and.returnValue(
        throwError(() => new Error('fail!'))
      );

      component.deleteTodo();

      expect(mockTodoService.triggerRefresh).not.toHaveBeenCalled();
    });
  });
});
