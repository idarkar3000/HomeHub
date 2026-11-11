import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Router } from '@angular/router';
import { Room, RoomRequest } from '../../../core/models/room.model';
import { TaskItem } from '../../../core/models/task.model';
import { RoomService } from '../../../core/services/room.service';
import { TaskService } from '../../../core/services/task.service';
import { ConfirmDialog, ConfirmDialogData } from '../../../shared/confirm-dialog/confirm-dialog';
import { EmptyState } from '../../../shared/empty-state/empty-state';
import { RoomFormDialog, RoomFormDialogData } from '../room-form-dialog/room-form-dialog';

@Component({
  selector: 'app-rooms-list',
  imports: [
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatCheckboxModule,
    MatProgressBarModule,
    EmptyState,
  ],
  templateUrl: './rooms-list.html',
  styleUrl: './rooms-list.scss',
})
export class RoomsList implements OnInit {
  private readonly roomService = inject(RoomService);
  private readonly taskService = inject(TaskService);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);

  readonly rooms = this.roomService.rooms;
  readonly tasks = signal<TaskItem[]>([]);

  readonly today = new Date();

  readonly allTodayTasks = computed(() =>
    this.tasks().filter((task) => this.isSameDay(task.dueDate, this.today)),
  );

  readonly todayDone = computed(
    () => this.allTodayTasks().filter((task) => task.isCompleted).length,
  );

  readonly ringRadius = 27;
  readonly ringCircumference = 2 * Math.PI * this.ringRadius;
  readonly ringOffset = computed(() => {
    const total = this.allTodayTasks().length;
    const ratio = total === 0 ? 0 : this.todayDone() / total;
    return this.ringCircumference * (1 - ratio);
  });

  private readonly accents = [
    '#e35d6a',
    '#e8833a',
    '#c9a227',
    '#5ba85f',
    '#3a9ea5',
    '#4a7fd6',
    '#8461d9',
    '#c95d9e',
  ];

  readonly headline = computed(() => {
    const pending = this.allTodayTasks().filter((task) => !task.isCompleted).length;
    if (pending > 0) {
      return `Te quedan ${pending} ${pending === 1 ? 'tarea' : 'tareas'} para hoy`;
    }
    if (this.allTodayTasks().length > 0) {
      return 'Todo listo por hoy';
    }
    return 'Hoy no tienes nada agendado';
  });

  readonly todayTasks = computed(() =>
    this.tasks().filter((task) => !task.isCompleted && this.isSameDay(task.dueDate, this.today)),
  );

  readonly completedCount = computed(
    () => this.tasks().filter((task) => task.isCompleted).length,
  );

  readonly nextTask = computed(() => {
    const todayKey = this.dateKey(this.today);
    return this.tasks()
      .filter((task) => !task.isCompleted && task.dueDate !== null && task.dueDate! >= todayKey)
      .sort((a, b) => a.dueDate!.localeCompare(b.dueDate!))[0];
  });

  readonly pendingByRoom = computed(() => {
    const counts = new Map<number, number>();
    for (const task of this.tasks()) {
      if (!task.isCompleted) {
        counts.set(task.roomId, (counts.get(task.roomId) ?? 0) + 1);
      }
    }
    return counts;
  });

  readonly doneByRoom = computed(() => {
    const counts = new Map<number, number>();
    for (const task of this.tasks()) {
      if (task.isCompleted) {
        counts.set(task.roomId, (counts.get(task.roomId) ?? 0) + 1);
      }
    }
    return counts;
  });

  readonly totalPending = computed(
    () => this.tasks().filter((task) => !task.isCompleted).length,
  );

  ngOnInit(): void {
    this.refresh();
  }

  pendingCount(roomId: number): number {
    return this.pendingByRoom().get(roomId) ?? 0;
  }

  accentFor(roomId: number): string {
    return this.accents[roomId % this.accents.length];
  }

  roomProgress(roomId: number): number {
    const pending = this.pendingByRoom().get(roomId) ?? 0;
    const done = this.doneByRoom().get(roomId) ?? 0;
    const total = pending + done;
    return total === 0 ? 0 : Math.round((done / total) * 100);
  }

  nextRoomTask(roomId: number): TaskItem | undefined {
    return this.tasks()
      .filter((task) => task.roomId === roomId && !task.isCompleted && task.dueDate !== null)
      .sort((a, b) => a.dueDate!.localeCompare(b.dueDate!))[0];
  }

  roomName(roomId: number): string {
    return this.rooms().find((room) => room.id === roomId)?.name ?? '';
  }

  toggleTodayTask(task: TaskItem): void {
    this.taskService.toggle(task.id).subscribe(() => this.refresh());
  }

  openRoom(room: Room): void {
    this.router.navigate(['/rooms', room.id]);
  }

  addRoom(): void {
    this.dialog
      .open<RoomFormDialog, RoomFormDialogData | undefined, RoomRequest>(RoomFormDialog, {
        width: '380px',
        maxWidth: '94vw',
      })
      .afterClosed()
      .subscribe((request) => {
        if (request) {
          this.roomService.create(request).subscribe(() => this.refresh());
        }
      });
  }

  editRoom(room: Room, event: Event): void {
    event.stopPropagation();
    this.dialog
      .open<RoomFormDialog, RoomFormDialogData, RoomRequest>(RoomFormDialog, {
        width: '380px',
        maxWidth: '94vw',
        data: { room },
      })
      .afterClosed()
      .subscribe((request) => {
        if (request) {
          this.roomService.update(room.id, request).subscribe(() => this.refresh());
        }
      });
  }

  deleteRoom(room: Room, event: Event): void {
    event.stopPropagation();
    this.dialog
      .open<ConfirmDialog, ConfirmDialogData, boolean>(ConfirmDialog, {
        data: {
          title: 'Eliminar habitación',
          message: `¿Seguro que quieres eliminar "${room.name}" y todas sus tareas?`,
        },
      })
      .afterClosed()
      .subscribe((confirmed) => {
        if (confirmed) {
          this.roomService.delete(room.id).subscribe(() => this.refresh());
        }
      });
  }

  private isSameDay(dueDate: string | null, date: Date): boolean {
    if (!dueDate) {
      return false;
    }
    return dueDate.substring(0, 10) === this.dateKey(date);
  }

  private dateKey(date: Date): string {
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${date.getFullYear()}-${month}-${day}`;
  }

  private refresh(): void {
    this.roomService.loadAll().subscribe();
    this.taskService.getAll().subscribe((tasks) => this.tasks.set(tasks));
  }
}
