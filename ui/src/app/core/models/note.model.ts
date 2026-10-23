export type NoteKind = 'note' | 'shopping';

export interface Note {
  id: number;
  content: string;
  isDone: boolean;
  kind: NoteKind;
  roomId: number | null;
  categoryId: number | null;
  createdAt: string;
}

export interface NoteRequest {
  content: string;
  kind: NoteKind;
  roomId: number | null;
  categoryId: number | null;
}
