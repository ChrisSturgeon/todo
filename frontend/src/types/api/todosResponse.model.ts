import { Todo } from './todo.model';

export type TodosResponse = {
  items: Todo[];
  totalCount: number;
};
