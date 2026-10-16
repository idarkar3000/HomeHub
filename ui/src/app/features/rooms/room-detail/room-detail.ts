import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleChange, MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router } from '@angular/router';
import { Room } from '../../../core/models/room.model';
import { RecurrenceType, TaskFilter, TaskItem, TaskRequest } from '../../../core/models/task.model';
import { RoomService } from '../../../core/services/room.service';
import { TaskService } from '../../../core/services/task.service';
import { ConfirmDialog, ConfirmDialogData } from '../../../shared/confirm-dialog/confirm-dialog';
import { EmptyState } from '../../../shared/empty-state/empty-state';
import { TaskFormDialog, TaskFormDialogData } from '../../tasks/task-form-dialog/task-form-dialog';

@Component({
  selector: 'app-room-detail',
  imports: [
    DatePipe,
    MatButtonModule,
    MatButtonToggleModule,
    MatCheckboxModule,
    MatIconModule,
    EmptyState,
  ],
  templateUrl: './room-detail.html',
  styleUrl: './room-detail.scss',
})
export class RoomDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly roomService = inject(RoomService);
  private readonly taskService = inject(TaskService);
  private readonly dialog = inject(MatDialog);

  private roomId = 0;

  readonly room = signal<Room | null>(null);
  readonly tasks = this.taskService.tasks;
  readonly filter = signal<TaskFilter>('all');

  readonly filteredTasks = computed(() => {
    const filter = this.filter();
    return this.tasks().filter((task) => {
      if (filter === 'pending') return !task.isCompleted;
      if (filter === 'completed') return task.isCompleted;
      return true;
    });
  });

  readonly pendingCount = computed(() => this.tasks().filter((task) => !task.isCompleted).length);

  readonly emptyTitle = computed(() => {
    switch (this.filter()) {
      case 'pending':
        return 'Nada pendiente';
      case 'completed':
        return 'Aún nada completado';
      default:
        return 'Sin tareas por ahora';
    }
  });

  readonly emptyMessage = computed(() => {
    switch (this.filter()) {
      case 'pending':
        return '¡Buen trabajo! Has completado todas las tareas.';
      case 'completed':
        return 'Marca tareas como completadas para verlas aquí.';
      default:
        return 'Añade la primera tarea de esta habitación.';
    }
  });

  ngOnInit(): void {
    this.roomId = Number(this.route.snapshot.paramMap.get('id'));
    this.roomService.getById(this.roomId).subscribe((room) => this.room.set(room));
    this.refreshTasks();
  }

  goBack(): void {
    this.router.navigate(['/rooms']);
  }

  onFilterChange(event: MatButtonToggleChange): void {
    this.filter.set(event.value as TaskFilter);
  }

  hasTime(task: TaskItem): boolean {
    return (
      !!task.dueDate && (!!task.endDate || task.dueDate.substring(11, 19) !== '00:00:00')
    );
  }

  recurrenceLabel(recurrence: RecurrenceType): string {
    switch (recurrence) {
      case 'daily':
        return 'Diaria';
      case 'weekly':
        return 'Semanal';
      case 'monthly':
        return 'Mensual';
      default:
        return '';
    }
  }

  toggle(task: TaskItem): void {
    this.taskService.toggle(task.id).subscribe(() => this.refreshTasks());
  }

  addTask(): void {
    this.dialog
      .open<TaskFormDialog, TaskFormDialogData, TaskRequest>(TaskFormDialog, {
        width: '380px',
        maxWidth: '94vw',
        data: { roomId: this.roomId },
      })
      .afterClosed()
      .subscribe((request) => {
        if (request) {
          this.taskService.create(this.roomId, request).subscribe(() => this.refreshTasks());
        }
      });
  }

  editTask(task: TaskItem): void {
    this.dialog
      .open<TaskFormDialog, TaskFormDialogData, TaskRequest>(TaskFormDialog, {
        width: '380px',
        maxWidth: '94vw',
        data: { roomId: this.roomId, task },
      })
      .afterClosed()
      .subscribe((request) => {
        if (request) {
          this.taskService.update(task.id, request).subscribe(() => this.refreshTasks());
        }
      });
  }

  deleteTask(task: TaskItem): void {
    this.dialog
      .open<ConfirmDialog, ConfirmDialogData, boolean>(ConfirmDialog, {
        data: {
          title: 'Eliminar tarea',
          message: `¿Seguro que quieres eliminar "${task.title}"?`,
        },
      })
      .afterClosed()
      .subscribe((confirmed) => {
        if (confirmed) {
          this.taskService.delete(task.id).subscribe(() => this.refreshTasks());
        }
      });
  }

  private refreshTasks(): void {
    this.taskService.loadByRoom(this.roomId).subscribe();
  }
}
