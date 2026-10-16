export type RecurrenceType = 'none' | 'daily' | 'weekly' | 'monthly';

export interface TaskItem {
  id: number;
  roomId: number;
  title: string;
  isCompleted: boolean;
  dueDate: string | null;
  endDate: string | null;
  recurrence: RecurrenceType;
}

export interface TaskRequest {
  roomId: number;
  title: string;
  isCompleted: boolean;
  dueDate: string | null;
  endDate: string | null;
  recurrence: RecurrenceType;
}

export type TaskFilter = 'all' | 'pending' | 'completed';
