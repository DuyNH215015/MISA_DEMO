//format số về dạng x,xx
export function formatNumber(value, precision = 2) {
  const num = Number(value)
  return isNaN(num) ? '--' : num.toFixed(precision)
}
// forrmat date về dạng dd/MM/YYYY
export function formatDate(value) {
  const date = new Date(value)
  if (isNaN(date)) return '-'
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()

  return `${day}/${month}/${year}`
}
//format dạng HH:MM:ss về HH:Mm
export const formatTimeToHM = (time) => {
  if (!time) return null
  const [h, m] = time.split(':')
  return `${h}:${m}`
}

// format về dạng HH:MM:SS để gửi lên BE
export const toTimeWithSeconds = (value) => {
  if (!value) return null
  // đã đúng dạng HH:mm:ss
  if (/^\d{2}:\d{2}:\d{2}$/.test(value)) {
    return value
  }
  // dạng HH:mm
  if (/^\d{1,2}:\d{2}$/.test(value)) {
    const [h, m] = value.split(':')
    return `${h.padStart(2, '0')}:${m}:00`
  }

  return null
}
// hàm thực thi format
// đầu vào date form dạng HH:MM
// đầu ra HH:MM:SS
export const normalizeShiftPayload = (form) => ({
  ...form,
  BeginShiftTime: toTimeWithSeconds(form.BeginShiftTime),
  EndShiftTime: toTimeWithSeconds(form.EndShiftTime),
  BeginBreakTime: toTimeWithSeconds(form.BeginBreakTime),
  EndBreakTime: toTimeWithSeconds(form.EndBreakTime),
})
