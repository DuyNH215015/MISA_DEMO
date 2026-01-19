export function toMinutes(time) {
  if (!time) return 0
  const [h, m] = time.split(':').map(Number)
  return h * 60 + m
}

export const validateForm = (form) => {
  // 1. bắt buộc
  if (!form.ShiftCode?.trim()) {
    return 'Mã ca không được để trống'
  }

  if (!form.ShiftName?.trim()) {
    return 'Tên ca không được để trống'
  }

  if (!form.BeginShiftTime || !form.EndShiftTime) {
    return 'Giờ vào ca và giờ hết ca là bắt buộc'
  }

  const beginShift = toMinutes(form.BeginShiftTime)
  const endShift = toMinutes(form.EndShiftTime)

  // 2. ca 0 phút ❌
  if (beginShift === endShift) {
    return 'Giờ vào ca và giờ hết ca không được trùng nhau'
  }

  // 3. xác định ca qua đêm
  const isOverNight = beginShift > endShift

  // 4. validate giờ nghỉ
  const hasBreak = form.BeginBreakTime || form.EndBreakTime

  if (hasBreak) {
    if (!form.BeginBreakTime || !form.EndBreakTime) {
      return 'Phải nhập đầy đủ giờ nghỉ'
    }

    const beginBreak = toMinutes(form.BeginBreakTime)
    const endBreak = toMinutes(form.EndBreakTime)

    if (beginBreak === endBreak) {
      return 'Giờ nghỉ không hợp lệ'
    }

    if (!isOverNight) {
      // ca trong ngày
      if (beginBreak < beginShift || endBreak > endShift) {
        return 'Thời gian nghỉ phải nằm trong thời gian làm'
      }
    } else {
      // ca qua đêm
      const breakInSameDay = beginBreak >= beginShift && beginBreak < 1440
      const breakInNextDay = endBreak <= endShift && endBreak > 0

      if (!(breakInSameDay || breakInNextDay)) {
        return 'Thời gian nghỉ không hợp lệ cho ca qua đêm'
      }
    }
  }
  return null // ✅ hợp lệ
}
