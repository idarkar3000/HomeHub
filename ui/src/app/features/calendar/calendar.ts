import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
  computed,
  inject,
  signal,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { FullCalendarModule } from '@fullcalendar/angular';
import {
  CalendarOptions,
  EventApi,
  EventClickArg,
  EventDropArg,
  EventInput,
} from '@fullcalendar/core';
import esLocale from '@fullcalendar/core/locales/es';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin, {
  Draggable,
  EventReceiveArg,
  EventResizeDoneArg,
} from '@fullcalendar/interaction';
import timeGridPlugin from '@fullcalendar/timegrid';
import { TaskItem, TaskRequest } from '../../core/models/task.model';
import { TaskService } from '../../core/services/task.service';
import { TaskFormDialog, TaskFormDialogData } from '../tasks/task-form-dialog/task-form-dialog';

@Component({
  selector: 'app-calendar',
  imports: [FullCalendarModule, MatIconModule],
  templateUrl: './calendar.html',
  styleUrl: './calendar.scss',
})
export class CalendarPage implements OnInit, AfterViewInit {
  private readonly taskService = inject(TaskService);
  private readonly dialog = inject(MatDialog);

  @ViewChild('unscheduledList') private readonly unscheduledListRef?: ElementRef<HTMLElement>;

  private readonly tasks = signal<TaskItem[]>([]);

  readonly scheduledCount = computed(() => this.tasks().filter((task) => task.dueDate !== null).length);
  readonly unscheduledTasks = computed(() =>
    this.tasks().filter((task) => task.dueDate === null && !task.isCompleted),
  );

  readonly calendarOptions = computed<CalendarOptions>(() => ({
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    initialView: 'dayGridMonth',
    locale: esLocale,
    firstDay: 1,
    height: 'auto',
    nowIndicator: true,
    allDaySlot: true,
    slotDuration: '00:30:00',
    snapDuration: '00:15:00',
    scrollTime: '07:00:00',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,timeGridDay',
    },
    editable: true,
    droppable: true,
    dayMaxEvents: true,
    events: this.tasks()
      .filter((task) => task.dueDate !== null)
      .map((task) => this.toEvent(task)),
    eventClick: (arg) => this.openTask(arg),
    eventDrop: (arg) => this.reschedule(arg),
    eventResize: (arg) => this.reschedule(arg),
    eventReceive: (arg) => this.receive(arg),
  }));

  ngOnInit(): void {
    this.refresh();
  }

  ngAfterViewInit(): void {
    const element = this.unscheduledListRef?.nativeElement;
    if (element) {
      new Draggable(element, {
        itemSelector: '.unscheduled-item',
        eventData: (el) => {
          const taskId = Number(el.getAttribute('data-task-id'));
          const task = this.tasks().find((item) => item.id === taskId);
          return { title: task?.title ?? '', extendedProps: { taskId } };
        },
      });
    }
  }

  private toEvent(task: TaskItem): EventInput {
    const timed = this.isTimed(task);
    return {
      id: String(task.id),
      title: task.title,
      start: task.dueDate ?? undefined,
      end: timed ? task.endDate ?? undefined : undefined,
      allDay: !timed,
      classNames: task.isCompleted ? ['event-completed'] : [],
      extendedProps: { taskId: task.id },
    };
  }

  private isTimed(task: TaskItem): boolean {
    if (!task.dueDate) {
      return false;
    }
    return !!task.endDate || task.dueDate.substring(11, 19) !== '00:00:00';
  }

  private findTask(arg: { event: EventApi }): TaskItem | undefined {
    const taskId = arg.event.extendedProps['taskId'];
    return this.tasks().find((item) => item.id === taskId);
  }

  private openTask(arg: EventClickArg): void {
    const task = this.findTask(arg);
    if (!task) {
      return;
    }

    this.dialog
      .open<TaskFormDialog, TaskFormDialogData, TaskRequest>(TaskFormDialog, {
        width: '380px',
        maxWidth: '94vw',
        data: { roomId: task.roomId, task },
      })
      .afterClosed()
      .subscribe((request) => {
        if (request) {
          this.taskService.update(task.id, request).subscribe(() => this.refresh());
        }
      });
  }

  private reschedule(arg: EventDropArg | EventResizeDoneArg): void {
    const task = this.findTask(arg);
    const start = arg.event.start;
    if (!task || !start) {
      arg.revert();
      return;
    }
    this.saveSchedule(task, start, arg.event.end, arg.event.allDay);
  }

  private receive(arg: EventReceiveArg): void {
    const task = this.findTask(arg);
    const start = arg.event.start;
    arg.event.remove();
    if (!task || !start) {
      return;
    }
    this.saveSchedule(task, start, arg.event.end, arg.event.allDay);
  }

  private saveSchedule(task: TaskItem, start: Date, end: Date | null, allDay: boolean): void {
    const request: TaskRequest = {
      roomId: task.roomId,
      title: task.title,
      isCompleted: task.isCompleted,
      dueDate: allDay ? this.toDateString(start) : this.toDateTimeString(start),
      endDate: allDay ? null : this.toDateTimeString(end ?? new Date(start.getTime() + 3_600_000)),
      recurrence: task.recurrence,
    };

    this.taskService.update(task.id, request).subscribe(() => this.refresh());
  }

  private toDateString(date: Date): string {
    return `${this.toDatePart(date)}T00:00:00`;
  }

  private toDateTimeString(date: Date): string {
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${this.toDatePart(date)}T${hours}:${minutes}:00`;
  }

  private toDatePart(date: Date): string {
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${date.getFullYear()}-${month}-${day}`;
  }

  private refresh(): void {
    this.taskService.getAll().subscribe((tasks) => this.tasks.set(tasks));
  }
}
