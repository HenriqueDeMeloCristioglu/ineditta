import NotificationService from "../../../../../utils/notifications/notification.service";
import Result from "../../../../../core/result";

export async function uploadFile(docSindService) {
  const arquivo = document.getElementById("file").files[0];
  const params = {
    arquivo,
  };

  const responseData = await docSindService.uploadFile(params);

  if (responseData.isFailure()) {
    NotificationService.error({ title: responseData.error });
    return Result.failure();
  }

  const fileName = responseData.value?.result.fileName;

  return Result.success({ fileName });
}
