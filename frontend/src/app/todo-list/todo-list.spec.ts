import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TodoList } from './todo-list';
import { TodoService } from '../todo-service/todo.service';
import { of, Subject } from 'rxjs';
import { TodosResponse } from '../../../api-types/api.types';
describe('TodoList', () => {
  let component: TodoList;
  let fixture: ComponentFixture<TodoList>;
  let todosSubject: Subject<any>;

  const fakeTodosResponse: TodosResponse = {
    items: [
      {
        id: '1234',
        name: 'Walk the dog',
        description: 'Take the dog for a walk',
        completed: true,
        position: 0,
        createdAt: 'now',
        updatedAt: 'now',
      },
      {
        id: '5678',
        name: 'Spring clean',
        description: 'Deep clean the entire house',
        completed: false,
        position: 1,
        createdAt: 'now',
        updatedAt: 'now',
      },
    ],
    totalCount: 2,
  };

  class mockTodoService {
    todos$ = todosSubject.asObservable();
    reorderTodos = jasmine.createSpy('reorderTodos').and.returnValue(of(null));
    triggerRefresh = jasmine.createSpy('triggerRefresh');
  }

  beforeEach(async () => {
    todosSubject = new Subject();

    await TestBed.configureTestingModule({
      imports: [TodoList],
      providers: [{ provide: TodoService, useClass: mockTodoService }],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('markup', () => {
    it('render content in an HTML section', () => {
      const htmlSection: HTMLElement =
        fixture.nativeElement.querySelector('#todos-list');

      expect(htmlSection).not.toBeNull();
      expect(htmlSection.tagName).toEqual('SECTION');
    });

    it('should have a semantic link between the section and heading', () => {
      const heading: HTMLHeadingElement =
        fixture.nativeElement.querySelector('h2');
      const section: HTMLElement =
        fixture.nativeElement.querySelector('#todos-list');

      expect(section.getAttribute('aria-labelledby')).toEqual(heading.id);
    });

    it('should display "Loading todos..." when the todos are loading', () => {
      fixture.detectChanges();

      expect(component.isLoading).toBeTrue();

      const loadingText: HTMLParagraphElement =
        fixture.nativeElement.querySelector('p');

      expect(loadingText.textContent).toContain('Loading todos...');
    });

    it('Should display "An error occurred retrieving the todos" when errored', () => {
      fixture.detectChanges();

      todosSubject.error(new Error('Network failed'));

      fixture.detectChanges();

      component.isLoading = false;
      component.isError = true;
      expect(component.isError).toBeTrue();

      fixture.detectChanges();

      const errorText: HTMLParagraphElement =
        fixture.nativeElement.querySelector('p');

      expect(errorText).not.toBeNull();

      expect(errorText.textContent).toContain(
        'An error occurred retrieving todos'
      );
    });

    it('should render a list when the todos are succesfully retrieved', () => {
      fixture.detectChanges();

      todosSubject.next(fakeTodosResponse);
      fixture.detectChanges();

      expect(component.isLoading).toBeFalse();
      expect(component.isError).toBeFalse();

      expect(component.todos.length).toBe(2);

      const list: HTMLUListElement = fixture.nativeElement.querySelector('ul');

      expect(list).not.toBeNull();

      const items: NodeListOf<HTMLLIElement> =
        list.querySelectorAll('app-todo-card');
      expect(items.length).toBe(2);

      expect(items[0].textContent).toContain('Walk the dog');
      expect(items[1].textContent).toContain('Spring clean');
    });
  });
});
