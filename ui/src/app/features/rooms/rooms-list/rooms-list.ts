import { Component, OnInit, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Router } from '@angular/router';
import { Room, RoomRequest } from '../../../core/models/room.model';
import { RoomService } from '../../../core/services/room.service';
import { ConfirmDialog, ConfirmDialogData } from '../../../shared/confirm-dialog/confirm-dialog';
import { EmptyState } from '../../../shared/empty-state/empty-state';
import { RoomFormDialog, RoomFormDialogData } from '../room-form-dialog/room-form-dialog';

@Component({
  selector: 'app-rooms-list',
  imports: [MatCardModule, MatButtonModule, MatIconModule, MatMenuModule, EmptyState],
  templateUrl: './rooms-list.html',
  styleUrl: './rooms-list.scss',
})
export class RoomsList implements OnInit {
  private readonly roomService = inject(RoomService);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);

  readonly rooms = this.roomService.rooms;

  ngOnInit(): void {
    this.refresh();
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

  private refresh(): void {
    this.roomService.loadAll().subscribe();
  }
}
