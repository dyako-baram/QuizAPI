using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.Data;
using QuizAPI.ModelDTO;
using QuizAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        #region Fields
        private readonly ApplicationDbContext _context;
        private readonly UserManager<QuizUser> _userManager;
        private readonly IMemoryCache _memoryCache;
        #endregion
        public QuizController(ApplicationDbContext context, 
            UserManager<QuizUser> userManager, 
            IMemoryCache memoryCache)
        {
            _context = context;
            _userManager = userManager;
            _memoryCache = memoryCache;
        }

        [HttpGet("[action]")]
        [Authorize(Roles ="Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ShowQuizzesDTO>>> GetAllQuizzesForAdminOnly()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await _userManager.GetUserAsync(User);
            List<ShowQuizzesDTO> result;//setup the caching
            result = _memoryCache.Get<List<ShowQuizzesDTO>>("GetAllQuizzesForAdminOnly");// get data from the memory cache

            if(result is null) // if there is no data then get it from the db and populate it to the cache
            {
                result = new();
                result = await _context.Quizzes.Select(x =>
                        new ShowQuizzesDTO
                        {
                            QuizId = x.QuizId,
                            Description = x.Description,
                            Duration = x.Duration,
                            CoverImage = x.CoverImage,
                            Genre = x.Genre,
                            IsPublished = x.IsPublished,
                            UserName = user.UserName,
                            Questions = x.Questions.Select(x => new QuestionDTO
                            {
                                QuestionId = x.QuestionId,
                                Description = x.Description,
                                Answers = x.Answers.Select(y => new AnswerDTO
                                {
                                    AnswerId = y.AnswerId,
                                    Description = y.Description,
                                    IsCorrectAnswer = y.IsCorrectAnswer,
                                }).ToList()
                            }).ToList()
                        })
                            .AsNoTracking().ToListAsync();
                _memoryCache.Set("GetAllQuizzesForAdminOnly", result, TimeSpan.FromSeconds(30));//30 seconds are just for example
            }
            return result;
        }

        [HttpGet("[action]")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ShowQuizzesDTO>>> GetAllApprovedQuizzes()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = await _userManager.GetUserAsync(User);
            List<ShowQuizzesDTO> result;
            result = _memoryCache.Get<List<ShowQuizzesDTO>>("GetAllApprovedQuizzes");// get data from the memory cache
            if (result is null) // if there is no data then get it from the db and populate it to the cache
            {
                
                result = await _context.Quizzes.Where(x => x.IsPublished == true).Select(x =>
                new ShowQuizzesDTO
                {
                    QuizId = x.QuizId,
                    Description = x.Description,
                    Duration = x.Duration,
                    CoverImage = x.CoverImage,
                    Genre = x.Genre,
                    IsPublished = x.IsPublished,
                    UserName = user.UserName,
                    Questions = x.Questions.Select(x => new QuestionDTO
                    {
                        QuestionId = x.QuestionId,
                        Description = x.Description,
                        Answers = x.Answers.Select(y => new AnswerDTO
                        {
                            AnswerId = y.AnswerId,
                            Description = y.Description,
                            IsCorrectAnswer = y.IsCorrectAnswer,
                        }).ToList()
                    }).ToList()
                })
                    .AsNoTracking().ToListAsync();
                _memoryCache.Set("GetAllApprovedQuizzes", result,TimeSpan.FromSeconds(30));//30 seconds are just for example
            }
            

            return result;
        }

        [HttpGet("[action]/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ShowQuizzesDTO>> GetQuizById([Required]int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = await _userManager.GetUserAsync(User);

            var result = await _context.Quizzes.AsNoTracking().Where(x=>x.QuizId==id).Select(x =>
            new ShowQuizzesDTO
            {
                QuizId = x.QuizId,
                Description = x.Description,
                Duration = x.Duration,
                CoverImage = x.CoverImage,
                Genre = x.Genre,
                IsPublished = x.IsPublished,
                UserName = user.UserName,
                Questions = x.Questions.Select(x => new QuestionDTO
                {
                    QuestionId = x.QuestionId,
                    Description = x.Description,
                    Answers = x.Answers.Select(y => new AnswerDTO
                    {
                        AnswerId = y.AnswerId,
                        Description = y.Description,
                        IsCorrectAnswer = y.IsCorrectAnswer,
                    }).ToList()
                }).ToList()
            })
                .AsNoTracking().FirstOrDefaultAsync();
            return result;
        }

        [HttpGet("[action]")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<string>>> GetAllQuizGenres()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var result=await _context.Quizzes.AsNoTracking().Select(x => x.Genre).ToListAsync();
            return result;
        }
        
        [HttpPost("[action]")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateQuiz(CreateQuizDTO createQuizDTO)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = await _userManager.GetUserAsync(User);
            if(user==null)
            {
                return BadRequest("User Doesnt Exist");
            }
            Quiz quizObj = new Quiz()
            {
                Description = createQuizDTO.Description,
                Genre = createQuizDTO.Genre,
                IsPublished = createQuizDTO.IsPublished,
                Duration = createQuizDTO.Duration,
                CoverImage = createQuizDTO.CoverImage,
                Questions = createQuizDTO.Questions.Select(x => new Question
                {
                    Description = x.Description,
                    Answers = x.Answers.Select(y => new Answer
                    {
                        Description = y.Description,
                        IsCorrectAnswer = y.IsCorrectAnswer,
                    }).ToList()

                }).ToList(),
                QuizUserId = user.Id,
            };

            var allQuestions = quizObj.Questions; // get the all of the questions
            var amountOfQuestions= allQuestions.Count(); //count the questions 
            var Answers = quizObj.Questions.Select(x=>x.Answers.Count()); // count the amount of answers each question has
            var doesEachQuestionHasOneAnswer = false;
            foreach(var item in Answers)
            {
                if (item < 2)// check each question whether they have less than 2 answers
                {
                    doesEachQuestionHasOneAnswer = true;
                }
            }
            if (amountOfQuestions < 1 || doesEachQuestionHasOneAnswer) // if we have zero question or a question with one answer return bad request
            {
                return BadRequest("Minimum of 1 Question And 2 Answers Per Question is Required");
            }
            var countAnswrersWithTrueValue = 0;
            foreach (var item in allQuestions)
            {
                //count all answers where the value is true
                countAnswrersWithTrueValue += item.Answers.Where(x => x.IsCorrectAnswer == true).Count();
            }
            if(countAnswrersWithTrueValue != amountOfQuestions) // if the amount of question doesnt match of the amount of correct answers meaning each question doesnt have one true question
            {
                return BadRequest("Each Question Must Have Only One true Answer");
            }

            await _context.Quizzes.AddAsync(quizObj);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("[action]/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> TakeAQuizById([Required]int id,QuestionAnswerDTO quizAnswers)
        {
            if (!User.Identity.IsAuthenticated) //check if user is authenticated
            {
                return Unauthorized();
            }
            var canQuizBeTaken = await CanUserTakeAQuiz(id); // can user take this quiz
            
            if (!canQuizBeTaken)
            {
                return BadRequest("No More Quizzes can Be Taken Today for this Quiz");
            }
            var user = await _userManager.GetUserAsync(User);// get current logged in user
            if (user == null)
            {
                return BadRequest("User Doesn't Exist");
            }
            
            var Quizes = await _context.Quizzes.Where(x => x.QuizId == id).AsNoTracking().FirstOrDefaultAsync();
            if(Quizes==null)
            {
                return NotFound("This Quiz Doesn't Exist");
            }
            if(!Quizes.IsPublished)
            {
                return BadRequest("This Quiz Is Not Published Yet, Please Try Again Later");
            }

            //get the time span (duration) of this question 
            var timeSpanForThisQuestion = _context.Answers.AsNoTracking().Select(x => x.Question).Select(x => x.Quiz).Where(x => x.QuizId == id).Select(x => x.Duration).FirstOrDefault();

            var timeUserStarted = quizAnswers.CurrentTime;
            var timeUserEnded = quizAnswers.TimeTaken;
            // get the difference between thoe to dates so we can compare them to the duration of the quiz
            var duration = timeUserEnded.Subtract(timeUserStarted);
            float successRate = 0.0f;
            
            //get the correct answers by the id that has been sent 
            var coorectAnswers = _context.Answers.Where(x => quizAnswers.ChosenAnswerIds.Contains(x.AnswerId)).Count(x=>x.IsCorrectAnswer==true);

            //check the quiz duration with the time taken by the quiz
            //TimeSpan.compare() return values are -1 if its shorter than duration, 0 if timespan is the same as duration, whereas 1 if timespan is longer than duration.
            if (TimeSpan.Compare(timeSpanForThisQuestion, duration) == 1 || TimeSpan.Compare(timeSpanForThisQuestion, duration) == 0)
            {
                successRate = (float)coorectAnswers / (float)quizAnswers.ChosenAnswerIds.Length;
            }
            else //user took more than allowed time
            {
                successRate = 0.0f;
            }

            await _context.QuizAttempts.AddAsync(new QuizAttempt
            {
                QuizUserId = user.Id,
                QuizId = id,
                StartDateTime = quizAnswers.CurrentTime,
                EndDateTime = quizAnswers.TimeTaken,
                SuccessRate = successRate
            });
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("[action]")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<UserQuizResult>>> GetUserQuizResult()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User Doesn't Exist");
            }
            var result = await _context.QuizAttempts.Where(x => x.QuizUserId == user.Id).Select(x => new UserQuizResult
            {
                QuizDescription= x.Quiz.Description,
                StartDateTime=x.StartDateTime,
                EndDateTime = x.EndDateTime,
                QuizSuccessRate=x.SuccessRate
            }).AsNoTracking().ToListAsync();
            return result;
        }
        private async Task<bool> CanUserTakeAQuiz(int id)
        {
            //to find out whether loged in user can take the same quiz or not we do the following 
            //get the logged in user and the attempts
            var user = await _userManager.GetUserAsync(User);
            var allAttempts = await _context.QuizAttempts.AsNoTracking().ToListAsync();

            if (allAttempts != null)
            {
                //get list of startdates of user with specific quiz
                var usersDates = allAttempts.Where(x => x.QuizUser.Id == user.Id && x.QuizId==id).Select(x => x.StartDateTime).ToList();

                var TimePlayedToday = 0;

                foreach (var item in usersDates)
                {
                    if (item.Date == DateTime.UtcNow.Date)//if the time matches today increase by 1
                    {
                        TimePlayedToday++;
                    }
                }
                if (TimePlayedToday < 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        [HttpGet("[action]/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> CanUserTakeAQuizToday([Required]int id)
        {
            return await CanUserTakeAQuiz(id);
        }

        [HttpDelete("[action]/{ids}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteQuizzesById([Required]int[] ids)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var listToDelete = new List<Quiz>();
            foreach (var item in ids)
            {
                listToDelete.Add(new Quiz() { QuizId = item });
            }
                       
            _context.Quizzes.RemoveRange(listToDelete);
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPut("[action]/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PublishAndUnpublishQuizById([Required]int id, PublishAndUnpublishQuizDTO publishAndUnpublishQuizDTO)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var result = await _context.Quizzes.FirstOrDefaultAsync(x => x.QuizId == id);
            if (result == null)
            {
                return NotFound("Quiz Doesn't Exist");
            }
            result.IsPublished=publishAndUnpublishQuizDTO.IsPublished;
            await _context.SaveChangesAsync();
            return Ok();

        }

        [HttpPut("[action]/{id}")]
        [Authorize(Roles ="Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateQuizById([Required]int id, UpdateQuizByIdDTO updateQuizByIdDTO)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var result = await _context.Quizzes.FirstOrDefaultAsync(x => x.QuizId == id);
            if (result==null)
            {
                return NotFound("This Quiz Doesn't Exist");
            }
            result.Description = updateQuizByIdDTO.Description;
            result.Genre = updateQuizByIdDTO.Genre;
            result.IsPublished = updateQuizByIdDTO.IsPublished;
            result.Duration = updateQuizByIdDTO.Duration;
            result.CoverImage = updateQuizByIdDTO.CoverImage;
            result.Questions = updateQuizByIdDTO.Questions.Select(x => new Question
            {
                Description = x.Description,
                Answers = x.Answers.Select(y => new Answer
                {
                    Description = y.Description,
                    IsCorrectAnswer = y.IsCorrectAnswer
                }
                ).ToList()
            }).ToList();
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateQuizOnlyById([Required]int id, UpdateQuizOnlyDTO updateQuizOnlyDTO)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var result = await _context.Quizzes.FirstOrDefaultAsync(x => x.QuizId == id);
            if (result == null)
            {
                return NotFound("This Quiz Doesn't Exist");
            }
            result.Description = updateQuizOnlyDTO.Description;
            result.Genre = updateQuizOnlyDTO.Genre;
            result.IsPublished = updateQuizOnlyDTO.IsPublished;
            result.Duration = updateQuizOnlyDTO.Duration;
            result.CoverImage = updateQuizOnlyDTO.CoverImage;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("[action]/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateQuestionOnlyById([Required]int id, UpdateQuestionDTO updateQuestionDTO)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var result = await _context.Questions.FirstOrDefaultAsync(x => x.QuestionId == id);
            if (result == null)
            {
                return NotFound("This Quiz Doesn't Exist");
            }
            result.Description = updateQuestionDTO.Description;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("[action]/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateAnswerOnlyById([Required]int id, UpdateAnswerDTO updateAnswerDTO)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var result = await _context.Answers.FirstOrDefaultAsync(x => x.AnswerId == id);
            if (result == null)
            {
                return NotFound("This Quiz Doesn't Exist");
            }
            result.Description = updateAnswerDTO.Description;
            result.IsCorrectAnswer = updateAnswerDTO.IsCorrectAnswer;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
